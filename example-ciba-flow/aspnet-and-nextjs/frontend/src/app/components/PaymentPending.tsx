import LoadingIndicator from "@/assets/logo/loading-indicator.svg";
import classes from "./PaymentPending.module.css";

export interface PaymentPendingProps {
  bindingMessage?: string;
}

const PaymentPending = ({ bindingMessage }: PaymentPendingProps) => {
  return (
    <div className="flex flex-col gap-4 items-center text-center">
      {bindingMessage && (
        <>
          <div>Please confirm the message using your BankID app.</div>
          <div className="bg-background-dark text-text-dark font-bold p-4 rounded-lg w-full">
            {bindingMessage}
          </div>
        </>
      )}
      {!bindingMessage && (
        <>
          <div>Please wait...</div>
          <LoadingIndicator className={classes.loading} />
        </>
      )}
    </div>
  );
};

export default PaymentPending;
