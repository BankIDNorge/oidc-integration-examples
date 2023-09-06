"use client";

import React, { useEffect, useMemo, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import CardContainer from "@/app/components/CardContainer";
import PaymentInitForm from "@/app/components/PaymentInitForm";
import { confirmPayment, pollResult } from "@/lib/api";
import PaymentPending from "@/app/components/PaymentPending";
import useSWR from "swr";
import { ErrorResponse, isError, PollSuccessResponse } from "@/lib/types";
import { getSignals } from "@/lib/signals";

export default function PaymentPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const amount = useMemo(() => searchParams.get("amount"), [searchParams]);
  const currency = useMemo(() => searchParams.get("currency"), [searchParams]);

  const [nnin, setNnin] = useState<string | undefined>();
  const [bindingMessage, setBindingMessage] = useState<string | undefined>();
  const [poll, setPoll] = useState(false);

  const { data: pollData, error: pollError } = useSWR<
    PollSuccessResponse,
    ErrorResponse
  >(poll ? "key" : null, pollResult);

  const handleRedirectSuccess = () => {
    router.push(`/success`);
  };

  const handleRedirectError = () => {
    router.push(`/error`);
  };

  const handleConfirmPayment = async () => {
    if (nnin && amount && currency) {
      const signals = await getSignals();
      const res = await confirmPayment({
        amount,
        currency,
        loginHint: {
          scheme: "nnin",
          value: nnin,
        },
        signals,
      });
      if (isError(res)) {
        handleRedirectError();
      } else {
        setBindingMessage(res.bindingMessage);
        setPoll(true);
      }
    }
  };

  useEffect(() => {
    handleConfirmPayment();
  }, [nnin, amount, currency]);

  useEffect(() => {
    console.log(pollData, pollError);
    if (pollData?.success) {
      setPoll(false);
      handleRedirectSuccess();
    } else if (
      pollError?.error &&
      pollError.error !== "authorization_pending"
    ) {
      setPoll(false);
      handleRedirectError();
    }
  }, [pollData, pollError]);

  return (
    <CardContainer>
      <div className="flex flex-row items-center">
        <h1 className="text-2xl font-bold flex-1">IBank</h1>
        <div className="text-center">Card Network</div>
      </div>
      <div className="my-6">
        <p className="font-bold">Authorization required</p>
        <div className="text-gray-300">
          TopShop <br />
          XXXX XXXX XXXX 1234 <br />
          Amount: {amount} {currency}
        </div>
      </div>
      {!nnin && <PaymentInitForm onApprove={setNnin} />}
      {nnin && <PaymentPending bindingMessage={bindingMessage} />}
    </CardContainer>
  );
}
