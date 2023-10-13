import fs from 'fs';
import { ZipFile } from "../scripts/zipFile";

const assert = require('assert');
describe("ZipFile tests", () => {
    it("Text file is not a ZIP", function () {
        fs.readFile('bestandje.txt', (_, data) => {
            const parts = [
                new Blob(['you construct a file...'], { type: 'text/plain' }),
                ' Same way as you do with blob',
                new Uint16Array(data)
            ];
            const file = new File(parts, "naam");
            const zip = new ZipFile(file);
            assert.equal(zip.fileName, "naam");
        });
    })
});