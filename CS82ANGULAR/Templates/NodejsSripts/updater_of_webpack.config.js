module.exports = (callback, code, fullPath, alias) => {

    if (typeof code === 'undefined') {
        throw new Error('The first input param with the name fullPath is not defined');
    }
    if (code === null) {
        throw new Error('The first input param with the name fullPath is null');
    }
    if (typeof fullPath === 'undefined') {
        throw new Error('The second input param with the name fullPath is not defined');
    }
    if (fullPath === null) {
        throw new Error('The second input param with the name fullPath is null');
    }
    if (typeof alias === 'undefined') {
        alias = fullPath;
    }
    if (alias === null) {
        alias = fullPath;
    }

    var lastName = '';
    let indexOfLastDelimeter = fullPath.lastIndexOf('/');
    if (indexOfLastDelimeter > -1) {
        lastName = fullPath.substring(indexOfLastDelimeter + 1);
    } else {
        lastName = fullPath;
    }

    const parserInst = require('@babel/parser');
    const babelInst = require('@babel/core');

    const ast = parserInst.parse(code, {
        sourceType: 'module',
        plugins: ['flow']
    });


    let isNotUpdated = true;
    let isExposesPresent = false;
    const StringLiteralVisitor = {
        StringLiteral(path) {
            if (path.node.value.endsWith('/' + lastName) || (path.node.value === lastName)) {
                path.node.value = fullPath;
                isNotUpdated = false;
            }
        }
    };
    const ObjectPropertyVisitor = {
        ObjectProperty(path) {
            if (path.node.value.value.endsWith('/' + lastName) || (path.node.value.value === lastName)) {
                path.node.value.value = fullPath;
                isNotUpdated = false;
            }
        }
    };
    const IdentifierVisitor = {
        Identifier(path) {
            if (path.node.name === 'exposes') {
                path.parentPath.traverse(ObjectPropertyVisitor);
                isExposesPresent = true;
            }
        }
    };

    babelInst.traverse(ast, IdentifierVisitor);

    if (isNotUpdated) {
        if (isExposesPresent) {
            babelInst.traverse(ast, {
                Identifier: function (path) {
                    if (path.node.name === 'exposes') {
                        path.parentPath.node.value.properties.push(babelInst.types.objectProperty(babelInst.types.stringLiteral(alias), babelInst.types.stringLiteral(fullPath)));
                    }
                },
            });
        } else {
            babelInst.traverse(ast, {
                CallExpression: function (path) {
                    if (path.node.callee.type === 'Identifier') {
                        if (path.node.callee.name === 'withModuleFederationPlugin') {
                            if (path.node.arguments.count < 1) {
                                path.node.arguments.push(
                                    babelInst.types.objectExpression([
                                        babelInst.types.objectProperty(
                                            // key
                                            babelInst.types.identifier('exposes'),
                                            // value
                                            babelInst.types.objectExpression([babelInst.types.objectProperty(babelInst.types.stringLiteral(alias), babelInst.types.stringLiteral(fullPath))])
                                        )
                                    ])
                                );
                            } else {
                                path.node.arguments = [
                                    babelInst.types.objectExpression([
                                        babelInst.types.objectProperty(
                                            // key
                                            babelInst.types.identifier('exposes'),
                                            // value
                                            babelInst.types.objectExpression([
                                                babelInst.types.objectProperty(babelInst.types.stringLiteral(alias), babelInst.types.stringLiteral(fullPath))
                                            ])
                                        )
                                    ])
                                ];
                            }
                        }
                    }
                }
            });
        }
    }
    const result = babelInst.transformFromAst(ast);
    callback(null, result.code);
}