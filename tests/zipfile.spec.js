"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const fs_1 = __importDefault(require("fs"));
const zipFile_1 = require("../scripts/zipFile");
const detectionResult_1 = require("../scripts/entities/detectionResult");
describe("ZipFile tests", () => {
    it("Text file should no be recognized as a ZIP-archive", () => __awaiter(void 0, void 0, void 0, function* () {
        const data = yield fs_1.default.promises.readFile('tests/files/TextFile.txt');
        const parts = [new Uint8Array(data)];
        const file = new File(parts, "TextFile.txt");
        const zip = new zipFile_1.ZipFile(file);
        zip.validate()
            .then(result => expect(result).toBe(detectionResult_1.DetectionResult.noZipArchive));
    }));
    it("ZIP-archive should be recognized as a ZIP-archive", () => __awaiter(void 0, void 0, void 0, function* () {
        const data = yield fs_1.default.promises.readFile('tests/files/Sample.zip');
        const parts = [new Uint8Array(data)];
        const file = new File(parts, "Sample.zip");
        const zip = new zipFile_1.ZipFile(file);
        zip.validate()
            .then(result => expect(result).toBe(detectionResult_1.DetectionResult.validZipArchive));
    }));
});
//# sourceMappingURL=zipfile.spec.js.map