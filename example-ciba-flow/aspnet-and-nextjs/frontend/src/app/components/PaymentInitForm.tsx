import BankIDIconPurple from "@/assets/logo/DNA-element-purple.svg";
import React, { FormEvent, useState } from "react";
import { userCheck } from "@/lib/api";
import { isError } from "@/lib/types";

export interface PaymentInitFormProps {
  /**
   * Callback function that is called when the user approves the payment with
   * a valid nnin.
   * @param nnin User-provided national identity number.
   */
  onApprove(nnin: string): void;
}

const PaymentInitForm = ({ onApprove }: PaymentInitFormProps) => {
  const [nnin, setNnin] = useState("");
  const [showErrorNnin, setShowErrorNnin] = React.useState(false);

  const confirmPayment = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    // TODO: Response typings
    const data = await userCheck({
      scheme: "nnin",
      value: nnin.toString(),
    });
    if (isError(data) || !data.exists) {
      setShowErrorNnin(true);
      return;
    }

    onApprove(nnin);
  };

  return (
    <form className="flex flex-col gap-4" onSubmit={confirmPayment}>
      <div className="mb-6">
        <label className="block mb-2 text-sm font-medium" htmlFor="nnin">
          National identity number
        </label>
        <input
          className="bg-background-light-hover p-3 rounded-lg outline-none border-0 w-full"
          type="text"
          id="nnin"
          name="nnin"
          required
          pattern="[0-9]{1,12}"
          autoFocus
          value={nnin}
          onChange={(e) => setNnin(e.target.value)}
        />
      </div>
      <button
        type="submit"
        className="flex flex-row items-center bg-background-dark gap-2 hover:bg-background-dark-hover text-text-dark text-lg font-medium px-6 py-2 rounded-full"
      >
        <BankIDIconPurple className="w-8 h-8" />
        Approve with BankID
      </button>
      {showErrorNnin && (
        <p className="text-red-500 text-center">
          Please enter a valid national identity number.
        </p>
      )}
    </form>
  );
};

export default PaymentInitForm;
