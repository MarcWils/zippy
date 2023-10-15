"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DetectionResult = void 0;
class DetectionResult {
    static invalidZip() {
        return new DetectionResult();
    }
    get isValidZip() {
        return this.zipFile != null;
    }
}
exports.DetectionResult = DetectionResult;
//# sourceMappingURL=detectionResult.js.map