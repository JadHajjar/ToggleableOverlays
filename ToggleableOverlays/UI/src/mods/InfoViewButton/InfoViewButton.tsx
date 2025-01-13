import { bindValue, trigger, useValue } from "cs2/api";
import { Button } from "cs2/ui";
import { ModuleRegistryExtend } from "cs2/modding";
import { useState } from "react";
import style from "./InfoViewButton.module.scss";
import stack from "images/stack.svg";
import classNames from "classnames";
import mod from "../../../mod.json";

const InfoViewsEnabled$ = bindValue<boolean>(mod.id, "InfoViewsEnabled");
const activeInfoview$ = bindValue<any>("infoviews", "activeInfoview");

export const InfoViewButton: ModuleRegistryExtend = (Component) => {
  return (props) => {
    const { children, ...otherProps } = props || {};
    const InfoViewsEnabled = useValue(InfoViewsEnabled$);
    const activeInfoview = useValue(activeInfoview$);
    const [infoviewOpened, setInfoviewOpened] = useState(false);

    if (!infoviewOpened) {
      setInfoviewOpened(true);
      trigger("ToggleableOverlays", "InfoViewOpened");
    }

    return (
      <>
        {activeInfoview && (
          <Button
            variant="icon"
            className={classNames(style.button, InfoViewsEnabled && style.selected)}
            onSelect={() => trigger(mod.id, "SetInfoViewsEnabled", !InfoViewsEnabled)}
          >
            <div>
              <img style={{ maskImage: `url(${stack})` }} />
              Toggle Infoview Filter
            </div>
            <div className={style.checkbox}>{InfoViewsEnabled && <img style={{ maskImage: `url(Media/Glyphs/Checkmark.svg)` }} />}</div>
          </Button>
        )}
        <Component {...otherProps}>{children}</Component>
      </>
    );
  };
};
