import Toast from 'react-bootstrap/Toast';
import { WithChildren } from '../../../_metronic/helpers';
import { FC } from 'react';

type props = {
    message: string,
    show: boolean
}
const BasicToast: FC<props & WithChildren> = ({ message, show }) => {
  return ( show &&
    <Toast >
      <Toast.Body>{message}</Toast.Body>
    </Toast>
  );
}

export default BasicToast;