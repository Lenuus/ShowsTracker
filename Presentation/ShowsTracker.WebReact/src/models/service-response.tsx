export default class ServiceResponse<T> {
    public data!: T;
    public errorMessage: string | undefined;
    public isSuccesfull: boolean = true;
}

export class ServiceResponseWithoutData {
    public errorMessage: string | undefined;
    public isSuccesfull: boolean = true;
}