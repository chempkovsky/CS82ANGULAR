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
        exportItems: []
    };

    const StringLiteralVisitor = {
        StringLiteral(path) {
            if (this.item) {
                this.item.exportSource = path.node.value;
            }
        }
    };
    const ExportSpecifierVisitor = {
        ExportSpecifier(path) {
            if (this.item) {
                this.item.exportSubtype = 'ExportSpecifier';
                if (this.item.exportSpecifiers) {
                    this.item.exportSpecifiers.push({ localNm: path.node.local.name, exportedNm: path.node.exported.name })
                } else {
                    this.item.exportSpecifiers = [{ localNm: path.node.local.name, exportedNm: path.node.exported.name }]
                }
            }
        }
    }
    const ExportNamespaceSpecifierVisitor = {
        ExportNamespaceSpecifier(path) {
            if (this.item) {
                this.item.exportSubtype = 'ExportNamespaceSpecifier';
                this.item.exportNamespace = path.node.exported.name;
            }
        }
    }
    const ExportVisitor = {
        ExportAllDeclaration(path) {
            var exportItem = {
                exportType: 'ExportAllDeclaration',
            }
            path.traverse(StringLiteralVisitor, { item: exportItem });
            result.exportItems.push(exportItem);
        },
        ExportNamedDeclaration(path) {

            var exportItem = {
                exportType: 'ExportNamedDeclaration',
            }
            path.traverse(ExportSpecifierVisitor, { item: exportItem });
            path.traverse(ExportNamespaceSpecifierVisitor, { item: exportItem });
            path.traverse(StringLiteralVisitor, { item: exportItem });
            result.exportItems.push(exportItem);
        },
        ExportDefaultDeclaration(path) {
            var exportItem = {
                exportType: 'ExportDefaultDeclaration',
            }
            path.traverse(StringLiteralVisitor, { item: exportItem });
            result.exportItems.push(exportItem);
        },
    };
    babelInst.traverse(ast, ExportVisitor);

    callback(null, JSON.stringify(result));
}