export class LocalFileHeader {

    static signature: Uint8Array = Uint8Array.from([80, 75, 3, 4]);

    VersionNeededToExtract: number;
}