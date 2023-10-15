import { ZipAnalyzer } from "./zipAnalyzer";

const zipFileSelector = <HTMLInputElement>document.getElementById('zipFileSelector');
const zipFileContents = document.getElementById('zipFileContents');

if (zipFileSelector) {
    zipFileSelector.addEventListener('change', () => {
        onZipFileSelected(zipFileSelector.files);
    });
}

function onZipFileSelected(files: FileList) {
    zipFileContents.classList.remove('hidden');

    if (files.length !== 1) { return; }
    const zipFile = new ZipAnalyzer(files[0]);

    document.getElementById('zipFileName').innerText = zipFile.fileName;
    zipFile.validate()
        .then(result => {
            const outcome = result.isValidZip ? "Valid ZIP archive" : "Not a ZIP archive";
            document.getElementById('zipDetectionResult').innerText = outcome;
        });
}
