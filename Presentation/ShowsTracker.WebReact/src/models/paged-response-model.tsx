export class PagedResponseModel<T> {
    public currentPage!: number;
    public totalPage!: number;
    public pageSize!: number;
    public data!: T[];
    public totalCount!: number;
}