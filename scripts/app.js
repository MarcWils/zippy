"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const zipFile_1 = require("./zipFile");
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
    const zipFile = new zipFile_1.ZipFile(files[0]);
    document.getElementById('zipFileName').innerText = zipFile.fileName;
    zipFile.validate();
}
//# sourceMappingURL=app.js.map