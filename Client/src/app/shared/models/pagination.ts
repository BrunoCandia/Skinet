export type Pagination<T> = {
    pageIndex: number;
    pageSize: number;
    totalCount: number;
    data: T[];
}