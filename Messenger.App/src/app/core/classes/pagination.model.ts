export class PagedEntities<T> {
    public page: number;
    public pageSize: number;
    public totalSize: number;
    public entities: T[];
  
    constructor(page: number, pageSize: number, totalSize: number, entities: T[]) {
      this.page = page;
      this.pageSize = pageSize;
      this.totalSize = totalSize;
      this.entities = entities;
    }
  }
  