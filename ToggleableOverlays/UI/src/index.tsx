import { ModRegistrar } from "cs2/modding";
import { InfoViewButton } from "mods/InfoViewButton/InfoViewButton";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver/VanillaComponentResolver";

const register: ModRegistrar = (moduleRegistry) => {
  VanillaComponentResolver.setRegistry(moduleRegistry);

  moduleRegistry.extend(
    "game-ui/game/components/infoviews/infoview-menu.tsx",
    "InfoviewMenu",
    InfoViewButton
  );
};

export default register;
