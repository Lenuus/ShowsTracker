import { useSearchParams } from "react-router-dom";
import { PageTitle } from "../../../_metronic/layout/core"
import { useEffect, useState } from "react";
import { post } from "../../api/make-api-request-authorized";
import { ServiceResponseWithoutData } from "../../../models/service-response";

const ConfirmEmailPage = () => {
    const [query, setQuery] = useSearchParams();
    const [isLoading, setIsLoading] = useState(false);
    const [emailConfirmed, setEmailConfirmed] = useState(false);
    useEffect(() => { confirmEmail() }, []);
    
    const confirmEmail = () => {
        setIsLoading(true);
        const userId = query.get("userId");
        const code = query.get("code");

        post<ServiceResponseWithoutData>("/account/confirm-email", {
            userId,
            code
        }).then((resolve) => {
            if (resolve.data.isSuccesfull) {
                setEmailConfirmed(true);
            }
            else {
                setEmailConfirmed(false);
            }
            setIsLoading(false);
        }).catch((err) => {
            setEmailConfirmed(false);
            setIsLoading(false);
        })
    }
    
    return (
        <>
            {
                emailConfirmed && !isLoading ?
                    <>
                        <center><h1>Email Confirmed</h1></center>
                    </>
                    :
                    <>
                        <center><h1>Code is not valid</h1></center>
                    </>
            }
            {isLoading &&
                <>
                    <center><h1>Loading</h1></center>
                </>
            }
        </>
    )
}

export { ConfirmEmailPage }
