"use client";

import { useRouter } from "next/navigation";
import React from "react";
import CardContainer from "@/app/components/CardContainer";

const BasketPage = () => {
  const router = useRouter();

  const sendBasket = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const amount = encodeURIComponent(event.currentTarget.amount.value);
    const currency = encodeURIComponent(
      event.currentTarget.currency.value.toUpperCase(),
    );
    router.push(`/payment?amount=${amount}&currency=${currency}`);
  };

  return (
    <CardContainer>
      <form className="flex flex-col items-center gap-8" onSubmit={sendBasket}>
        <div className="text-center">
          <h1 className="text-2xl font-medium mb-1">TopShop</h1>
          <h2 className="text-gray-400">Checkout</h2>
        </div>
        <div>
          <label className="mb-2 block text-sm font-medium" htmlFor="amount">
            Amount
          </label>
          <input
            placeholder="100"
            className="bg-background-light-hover p-3 rounded-lg outline-none border-0"
            type="text"
            id="amount"
            name="Amount"
            required
            pattern="[0-9]{1,4}"
          />
        </div>
        <div>
          <label className="block mb-2 text-sm font-medium" htmlFor="currency">
            Currency
          </label>
          <input
            placeholder="NOK"
            className="bg-background-light-hover p-3 rounded-lg outline-none border-0"
            type="text"
            id="currency"
            name="Currency"
            required
            pattern="[a-zA-Z]{1,8}"
          />
        </div>
        <button
          className="flex-row bg-background-dark hover:bg-background-dark-hover text-text-dark text-lg font-medium px-6 py-2 rounded-full"
          type="submit"
        >
          Submit
        </button>
      </form>
    </CardContainer>
  );
};
export default BasketPage;
