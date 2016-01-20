![Alt text](http://g.gravizo.com/g?
    digraph G {
      edge[arrowhead="open" color="blue"];
      node [color="blue"];
      /* entities */
      CategoryGroup [label="Ценовая категория"];
      Project [label="Проект"];
      ProjectCategory [label="Рубрика проекта"];
      ProjectTerritory [label="Территория"];
      Firm [label="Фирма"];
      FirmCategory [label="Рубрика фирмы"];
      FirmAccount [label="Лицевой счет фирмы"];
      Client [label="Клиент"];
      Contact [label="Контакт клиента"];
      /* links */
      Project -> Firm;
      Project -> ProjectCategory;
      Project -> ProjectTerritory;
      Firm -> ProjectTerritory;
      Firm -> FirmCategory;
      Firm -> FirmAccount;
      Firm -> CategoryGroup; 
      Firm -> Client;
      FirmCategory -> ProjectCategory;
      Client -> Contact;
      Client -> CategoryGroup;
    }
)