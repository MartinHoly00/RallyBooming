import {
  BaywatchIcon,
  DefaultIcon,
  FormulaIcon,
  PoliceIcon,
  RaceIcon,
  VanIcon,
} from "../assets/iconIndex";

export type CarCard = {
  name: string;
  desc: string;
  icon: string;
};

export const cars: CarCard[] = [
  {
    name: "Baywatch",
    desc: "Baywatch car, very good big guy.",
    icon: BaywatchIcon,
  },
  {
    name: "Default car",
    desc: "Default car, decent driving.",
    icon: DefaultIcon,
  },
  {
    name: "Formula",
    desc: "Formula car, fastest on earth. Bzum bzum.",
    icon: FormulaIcon,
  },
  {
    name: "Police",
    desc: "Police car, it is police.",
    icon: PoliceIcon,
  },
  {
    name: "Race car",
    desc: "Very fast, very good.",
    icon: RaceIcon,
  },
  {
    name: "Van",
    desc: "Big, tanky, not so fast.",
    icon: VanIcon,
  },
];
