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

# Агрегаты
## Сущность OrderPosition
Строится на основании двух таблиц ERM: OrderPosition + OrderPositionAdvertisement. Пакеты представляются плоским списком. 
Могут повторяться записи с одинаковыми (OrderId, PackagePositionId, ItemPositionId) из-за того, что по одной OP есть продажи с разными объектами привязки.

Например, для пакета мы получим следующий набор записей:

OrderId | PackagePositionId | ItemPositionId | CategoryId | FirmAddressId | Комментарий
--- | --- | --- | --- | --- | ---
1 | 1 | 1 | null | null | на основании записи в OP
1 | 1 | 2 | x | x | на основании записи в OPA
1 | 1 | 2 | x | x | на основании записи в OPA
1 | 1 | 3 | x | x | на основании записи в OPA

А для простой:

OrderId | PackagePositionId | ItemPositionId | CategoryId | FirmAddressId | Комментарий
--- | --- | --- | --- | --- | ---
1 | 1 | 1 | x | x | на основании записи в OPA

где х может быть null или значением
