import fs from 'fs';
import { ZipAnalyzer } from "../scripts/zipAnalyzer";

describe("ZipFile tests", () => {
    it("Text file should no be recognized as a ZIP-archive", async () => {
        const data = await fs.promises.readFile('tests/files/TextFile.txt');

        const parts = [new Uint8Array(data)];
        const file = new File(parts, "TextFile.txt");
        const zip = new ZipAnalyzer(file);
        zip.validate()
            .then(result => expect(result.isValidZip).toBeFalsy());
    });

    it("ZIP-archive should be recognized as a ZIP-archive", async () => {
        const data = await fs.promises.readFile('tests/files/Sample.zip');

        const parts = [new Uint8Array(data)];
        const file = new File(parts, "Sample.zip");
        const zip = new ZipAnalyzer(file);
        zip.validate()
            .then(result => expect(result.isValidZip).toBeTruthy());
    });
});