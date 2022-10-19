module.exports = (callback, code) => {

    if (typeof code === 'undefined') {
        throw new Error('The first input param with the name fullPath is not defined');
    }
    if (code === null) {
        throw new Error('The first input param with the name fullPath is null');
    }

    const parserInst = require('@babel/parser')
    const babelInst = require("@babel/core");

    const ast = parserInst.parse(code, {
        sourceType: "module",
        plugins: ["flow"]
    });


    var result = {
        exposeItems: []
    };


    const ObjectPropertyVisitor = {
        ObjectProperty(path) {
            result.exposeItems.push(
                {
                    exposeKey: path.node.key.value,
                    exposeValue: path.node.value.value
                });
        }
    };
    const IdentifierVisitor = {
        Identifier(path) {
            if (path.node.name === 'exposes') {
                path.parentPath.traverse(ObjectPropertyVisitor);
            }
        }
    };

    babelInst.traverse(ast, IdentifierVisitor);

    callback(null, JSON.stringify(result));
}