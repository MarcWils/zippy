"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const fs_1 = __importDefault(require("fs"));
const zipFile_1 = require("../scripts/zipFile");
const assert = require('assert');
describe("ZipFile tests", () => {
    it("Text file is not a ZIP", function () {
        fs_1.default.readFile('bestandje.txt', (_, data) => {
            const parts = [
                new Blob(['you construct a file...'], { type: 'text/plain' }),
                ' Same way as you do with blob',
                new Uint16Array(data)
            ];
            const file = new File(parts, "naam");
            const zip = new zipFile_1.ZipFile(file);
            assert.equal(zip.fileName, "naam");
        });
    });
});
//# sourceMappingURL=zipfile.spec.js.map