import { Category } from "../enums/category-enum";
import { Status } from "../enums/status-enum";
import { PagedRequestModel } from "../paged-request-model";

export class ShowListRequestModel extends PagedRequestModel {
    public Search: string = "";
    public Categories: Category[] = [];
    public Statuses: Status[] = [];
    public Genres: string[] = [];
    public StartRating: number = 0;
    public EndRating: number = 10;
}