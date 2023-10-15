"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const zipAnalyzer_1 = require("./zipAnalyzer");
const zipFileSelector = document.getElementById('zipFileSelector');
const zipFileContents = document.getElementById('zipFileContents');
if (zipFileSelector) {
    zipFileSelector.addEventListener('change', () => {
        onZipFileSelected(zipFileSelector.files);
    });
}
function onZipFileSelected(files) {
    zipFileContents.classList.remove('hidden');
    if (files.length !== 1) {
        return;
    }
    const zipFile = new zipAnalyzer_1.ZipAnalyzer(files[0]);
    document.getElementById('zipFileName').innerText = zipFile.fileName;
    zipFile.validate()
        .then(result => {
        const outcome = result.isValidZip ? "Valid ZIP archive" : "Not a ZIP archive";
        document.getElementById('zipDetectionResult').innerText = outcome;
    });
}
//# sourceMappingURL=app.js.map