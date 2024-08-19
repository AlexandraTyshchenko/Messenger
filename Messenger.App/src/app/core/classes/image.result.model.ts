export class ImageResult {
    relativePath: string;
    fileName: string;

    constructor(relativePath: string, fileName: string) {
        this.relativePath = relativePath;
        this.fileName = fileName;
    }
}
