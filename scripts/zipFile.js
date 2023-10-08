"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ZipFile = void 0;
class ZipFile {
    constructor(file) {
        this.file = file;
        this.fileName = file.name;
    }
    validate() {
        const signature = this.file.slice(0, 4);
        signature.arrayBuffer()
            .then((buffer) => this.onSignatureRead(buffer));
    }
    onSignatureRead(signature) {
        console.log(this.toHexString(new Uint8Array(signature)));
    }
    toHexString(byteArray) {
        return [...byteArray]
            .map(x => x.toString(16).padStart(2, '0'))
            .join('');
    }
}
exports.ZipFile = ZipFile;
//# sourceMappingURL=zipFile.js.map