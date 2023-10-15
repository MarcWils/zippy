"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ZipAnalyzer = void 0;
const detectionResult_1 = require("./entities/detectionResult");
const zipFile_1 = require("./entities/zipFile");
const localFileHeader_1 = require("./zipformat/localFileHeader");
class ZipAnalyzer {
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
        if (this.signaturesMatch(localFileHeader_1.LocalFileHeader.signature, new Uint8Array(signature))) {
            return this.tryReadFirstLocalFileHeader();
        }
        else {
            return Promise.resolve(detectionResult_1.DetectionResult.invalidZip());
        }
    }
    tryReadFirstLocalFileHeader() {
        const lfh = this.file.slice(0, 14);
        return lfh.arrayBuffer()
            .then(() => {
            var result = new detectionResult_1.DetectionResult();
            result.zipFile = new zipFile_1.ZipFile();
            return result;
        });
    }
    signaturesMatch(a, b) {
        if (a.byteLength != b.byteLength) {
            return false;
        }
        return a.every((val, i) => val == b[i]);
    }
}
exports.ZipAnalyzer = ZipAnalyzer;
//# sourceMappingURL=zipAnalyzer.js.map