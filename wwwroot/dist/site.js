/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./scripts/entities/detectionResult.ts":
/*!*********************************************!*\
  !*** ./scripts/entities/detectionResult.ts ***!
  \*********************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.DetectionResult = void 0;
var DetectionResult;
(function (DetectionResult) {
    DetectionResult[DetectionResult["validZipArchive"] = 0] = "validZipArchive";
    DetectionResult[DetectionResult["noZipArchive"] = 1] = "noZipArchive";
})(DetectionResult || (exports.DetectionResult = DetectionResult = {}));


/***/ }),

/***/ "./scripts/zipFile.ts":
/*!****************************!*\
  !*** ./scripts/zipFile.ts ***!
  \****************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ZipFile = void 0;
const detectionResult_1 = __webpack_require__(/*! ./entities/detectionResult */ "./scripts/entities/detectionResult.ts");
class ZipFile {
    constructor(file) {
        this.file = file;
        this.fileName = file.name;
    }
    validate() {
        const signature = this.file.slice(0, 4);
        return signature.arrayBuffer()
            .then((buffer) => this.validateSignature(buffer));
    }
    validateSignature(signature) {
        if (this.signaturesMatch(ZipFile.zipSignature, new Uint8Array(signature))) {
            return detectionResult_1.DetectionResult.validZipArchive;
        }
        else {
            return detectionResult_1.DetectionResult.noZipArchive;
        }
    }
    signaturesMatch(a, b) {
        if (a.byteLength != b.byteLength) {
            return false;
        }
        return a.every((val, i) => val == b[i]);
    }
}
exports.ZipFile = ZipFile;
ZipFile.zipSignature = Uint8Array.from([80, 75, 3, 4]);


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
const detectionResult_1 = __webpack_require__(/*! ./entities/detectionResult */ "./scripts/entities/detectionResult.ts");
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
    zipFile.validate()
        .then(result => {
        const outcome = result == detectionResult_1.DetectionResult.validZipArchive ? "Valid ZIP archive" : "Not a ZIP achive";
        document.getElementById('zipDetectionResult').innerText = outcome;
    });
}

})();

/******/ })()
;
//# sourceMappingURL=site.js.map