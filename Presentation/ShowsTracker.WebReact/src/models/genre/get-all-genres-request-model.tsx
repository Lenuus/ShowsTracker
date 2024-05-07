import { PagedRequestModel } from "../paged-request-model";

export class GetAllGenresRequestModel extends PagedRequestModel {
    public search: string = "";
}