export class ZipFile {
    file: File;
    public fileName: string;

    constructor(file: File) {
        this.file = file;
        this.fileName = file.name;
    }

    public validate(): void{
        const signature = this.file.slice(0, 4);
        signature.arrayBuffer()
            .then((buffer) => this.onSignatureRead(buffer));
    }

    onSignatureRead(signature: ArrayBuffer) {
        console.log(this.toHexString(new Uint8Array(signature)));
    }

    private toHexString(byteArray: Uint8Array): string {
        return [...byteArray]
            .map(x => x.toString(16).padStart(2, '0'))
            .join('');
    }
}