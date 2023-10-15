import { ZipFile } from "./zipFile";

export class DetectionResult {
    static invalidZip(): DetectionResult {
        return new DetectionResult();
    }

    public get isValidZip(): boolean {
        return this.zipFile != null;
    }

    public zipFile: ZipFile;
}