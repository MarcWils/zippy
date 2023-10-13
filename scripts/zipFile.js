"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ZipFile = void 0;
const detectionResult_1 = require("./entities/detectionResult");
class ZipFile {
    constructor(file) {
        this.file = file;
        this.fileName = file.name;
    }
    validate() {
        const signature = this.file.slice(0, 4);
        return signature.arrayBuffer()
            .then((buffer) => this.validateSignature(buffer));
    }
    validateSignature(signature) {
        if (this.signaturesMatch(ZipFile.zipSignature, new Uint8Array(signature))) {
            return detectionResult_1.DetectionResult.validZipArchive;
        }
        else {
            return detectionResult_1.DetectionResult.noZipArchive;
        }
    }
    signaturesMatch(a, b) {
        if (a.byteLength != b.byteLength) {
            return false;
        }
        return a.every((val, i) => val == b[i]);
    }
}
exports.ZipFile = ZipFile;
ZipFile.zipSignature = Uint8Array.from([80, 75, 3, 4]);
//# sourceMappingURL=zipFile.js.map