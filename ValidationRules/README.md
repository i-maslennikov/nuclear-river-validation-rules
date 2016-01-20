![Alt text](http://g.gravizo.com/g?
digraph G {
  edge[arrowhead="open" color="blue"];
  node [color="blue"];
  rankdir=LR;
  MasterPositionGroup[color="red"]
  PositionCategory[color="red"]
  Order -> OrderPosition;
  Order -> OrderPrice;
  Order -> OrderPeriod;
  Price -> PricePosition;
  Price -> DeniedPosition;
  Price -> MasterPosition;
  Price -> OrderPrice;
  Price -> PricePeriod;
  Period -> OrderPeriod;
  Period -> PricePeriod;
  Position -> PositionCategory;
  Position -> PricePosition;
  Position -> DeniedPosition;
  Position -> MasterPosition;
  MasterPosition -> MasterPositionGroup;
  Position -> OrderPosition;
}
)