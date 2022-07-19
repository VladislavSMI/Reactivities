import React from "react";
import { useField } from "formik";
import { FormField, Label } from "semantic-ui-react";
import DatePicker, { ReactDatePickerProps } from "react-datepicker";

// Partial<ReactDatePickerProps> means that all prop types are optional, eg. in our case onChange prop for compulsory and we don't have to pass because we are using it in our DatePicker directly with helpers from Formik
export default function MyDateInput(props: Partial<ReactDatePickerProps>) {
  const [field, meta, helpers] = useField(props.name!);
  return (
    // css for date picker we can target it styles with .react-datepicker-wrapper {}
    <FormField error={meta.touched && !!meta.error}>
      <DatePicker
        {...field}
        {...props}
        selected={(field.value && new Date(field.value)) || null}
        onChange={(value) => helpers.setValue(value)}
      />
      {meta.touched && meta.error ? (
        <Label basic color="red">
          {meta.error}
        </Label>
      ) : null}
    </FormField>
  );
}
