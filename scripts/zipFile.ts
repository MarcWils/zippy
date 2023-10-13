import { DetectionResult } from "./entities/detectionResult";

export class ZipFile {
    file: File;
    public fileName: string;

    static zipSignature: Uint8Array = Uint8Array.from([80, 75, 3, 4]);

    constructor(file: File) {
        this.file = file;
        this.fileName = file.name;
    }

    public validate(): Promise<DetectionResult> {
        const signature = this.file.slice(0, 4);

        return signature.arrayBuffer()
            .then((buffer) => this.validateSignature(buffer));
    }

    validateSignature(signature: ArrayBuffer): DetectionResult {
        if (this.signaturesMatch(ZipFile.zipSignature, new Uint8Array(signature))) {
            return DetectionResult.validZipArchive;
        } else {
            return DetectionResult.noZipArchive;
        }
    }

    private signaturesMatch(a: Uint8Array, b: Uint8Array): boolean {
        if (a.byteLength != b.byteLength) {
            return false;
        }
        return a.every((val, i) => val == b[i]);
    }
}