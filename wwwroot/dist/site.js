/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./scripts/zipFile.ts":
/*!****************************!*\
  !*** ./scripts/zipFile.ts ***!
  \****************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ZipFile = void 0;
class ZipFile {
    constructor(file) {
        this.file = file;
        this.fileName = file.name;
    }
    validate() {
        const signature = this.file.slice(0, 4);
        signature.arrayBuffer()
            .then((buffer) => this.onSignatureRead(buffer));
    }
    onSignatureRead(signature) {
        console.log(this.toHexString(new Uint8Array(signature)));
    }
    toHexString(byteArray) {
        return [...byteArray]
            .map(x => x.toString(16).padStart(2, '0'))
            .join('');
    }
}
exports.ZipFile = ZipFile;


/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
var exports = __webpack_exports__;
/*!************************!*\
  !*** ./scripts/app.ts ***!
  \************************/

Object.defineProperty(exports, "__esModule", ({ value: true }));
const zipFile_1 = __webpack_require__(/*! ./zipFile */ "./scripts/zipFile.ts");
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

})();

/******/ })()
;
//# sourceMappingURL=site.js.map