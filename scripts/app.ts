import { ZipFile } from "./zipFile";

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
    const zipFile = new ZipFile(files[0]);

    document.getElementById('zipFileName').innerText = zipFile.fileName;
    zipFile.validate();
}