import { trigger } from "cs2/api";
import { ModuleRegistryExtend } from "cs2/modding";
import { useState } from "react";

export const InfoViewButton: ModuleRegistryExtend = (Component) => {
  return (props) => {
    const { children, ...otherProps } = props || {};

    const [infoviewOpened, setInfoviewOpened] = useState(false);

    if (!infoviewOpened) {
      setInfoviewOpened(true);
      trigger("ToggleableOverlays", "InfoViewOpened");
    }

    return (
      <>
        <Component {...otherProps}>{children}</Component>
      </>
    );
  };
};
