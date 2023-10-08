"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const zipFile_1 = require("../scripts/zipFile");
var assert = require('assert');
describe("ZipFile tests", () => {
    it("Test 1", function () {
        var file = new File([""], "naam");
        const zip = new zipFile_1.ZipFile(file);
        assert.ok("naam", zip.fileName);
    });
});
//# sourceMappingURL=zipfile.spec.js.map