import { DetectionResult } from "./entities/detectionResult";
import { ZipFile } from "./entities/zipFile";
import { LocalFileHeader } from "./zipformat/localFileHeader";

export class ZipAnalyzer {
    file: File;
    public fileName: string;
    constructor(file: File) {
        this.file = file;
        this.fileName = file.name;
    }

    public validate(): Promise<DetectionResult> {
        const signature = this.file.slice(0, 4);

        return signature.arrayBuffer()
            .then((buffer) => this.validateSignature(buffer));
    }

    private validateSignature(signature: ArrayBuffer): Promise<DetectionResult> {
        if (this.signaturesMatch(LocalFileHeader.signature, new Uint8Array(signature))) {
            return this.tryReadFirstLocalFileHeader();
        } else {
            return Promise.resolve(DetectionResult.invalidZip());
        }
    }

    private tryReadFirstLocalFileHeader(): Promise<DetectionResult> {
        const lfh = this.file.slice(0, 14);
        return lfh.arrayBuffer()
            .then(() => {
                var result = new DetectionResult();
                result.zipFile = new ZipFile();
                return result;
            });
    }

    private signaturesMatch(a: Uint8Array, b: Uint8Array): boolean {
        if (a.byteLength != b.byteLength) {
            return false;
        }
        return a.every((val, i) => val == b[i]);
    }
}