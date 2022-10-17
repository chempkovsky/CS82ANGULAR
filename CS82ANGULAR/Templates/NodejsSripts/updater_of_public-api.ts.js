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
    var isaliasDefined = false;
    if (!(typeof alias === 'undefined')) {
        if (!(alias === null)) {
            isaliasDefined = true;
        }
    }


    var lastName = '';
    let indexOfLastDelimeter = fullPath.lastIndexOf('/');
    if (indexOfLastDelimeter > -1) {
        lastName = fullPath.substring(indexOfLastDelimeter + 1);
    } else {
        lastName = fullPath;
    }

    const parserInst = require('@babel/parser')
    const babelInst = require("@babel/core");

    const ast = parserInst.parse(code, {
        sourceType: "module",
        plugins: ["flow"]
    });


    let isNotUpdated = true;

    const StringLiteralVisitor = {
        StringLiteral(path) {
            if (path.node.value.endsWith('/' + lastName) || (path.node.value === lastName)) {
                path.node.value = fullPath;
                isNotUpdated = false;
            }
        }
    };
    const ExportVisitor = {
        ExportAllDeclaration(path) {
            path.traverse(StringLiteralVisitor);
        },
        ExportNamedDeclaration(path) {
            path.traverse(StringLiteralVisitor);
        },
        ExportDefaultDeclaration(path) {
            path.traverse(StringLiteralVisitor);
        },
    };
    babelInst.traverse(ast, ExportVisitor);
    const ProgramVisitor = {
        Program(path) {
            if (path.node.body) {
                if (isaliasDefined) {
                    path.node.body.push(babelInst.types.exportNamedDeclaration(null, [babelInst.types.exportNamespaceSpecifier(babelInst.types.identifier(alias))], babelInst.types.stringLiteral(fullPath)));
                } else {
                    path.node.body.push(babelInst.types.exportAllDeclaration(babelInst.types.stringLiteral(fullPath)));
                }
            } else {
                if (isaliasDefined) {
                    path.node.body = [babelInst.types.exportNamedDeclaration(null, [babelInst.types.exportNamespaceSpecifier(babelInst.types.identifier(alias))], babelInst.types.stringLiteral(fullPath))];
                } else {
                    path.node.body.push(babelInst.types.exportAllDeclaration(babelInst.types.stringLiteral(fullPath)));
                }
            }
        }
    }
    if (isNotUpdated) {
        babelInst.traverse(ast, ProgramVisitor);
    }

    const result = babelInst.transformFromAst(ast);
    callback(null, result.code);
}