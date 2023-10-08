import { ZipFile } from "../scripts/zipFile";

var assert = require('assert');
describe("ZipFile tests", () => {
    it("Test 1", function () {
        var file = new File([""], "naam");
        const zip = new ZipFile(file);
        assert.ok("naam", zip.fileName);
    })
});