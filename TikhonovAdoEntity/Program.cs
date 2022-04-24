using TikhonovAdoEntity.Context;
using System.Data.SqlClient;

static void AdoNet() //использование AdoNet
{
    Console.WriteLine("--------------------------------------------------------------------------------------------");
    Console.WriteLine("AdoNet");
    Console.WriteLine();

    var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;");
    connection.Open();

    var sort1 = connection.CreateCommand(); //Вывести список товаров, отсортированных по названию своей группы, далее по цене.
    sort1.CommandText = "select p.Id, p.Name, p.Price, p.In_Stock, c.Category_Name from ProductOffer p join CategoryTable c on p.Category = c.Category order by Category_Name, Price";
    var reader1 = sort1.ExecuteReader();
    Console.WriteLine("Список товаров, отсортированных по названию своей группы, далее по цене:");
    while (reader1.Read())
    {
        Console.WriteLine("{0} {1} {2} {3} {4}", reader1.GetInt32(0), reader1.GetString(1), reader1.GetDouble(2), reader1.GetInt32(3), reader1.GetString(4));
    }
    reader1.Close();

    Console.WriteLine();
    var sort2 = connection.CreateCommand();//Вывести список товаров, отсортированных по своей суммарной стоимости на складе (единиц товара * цена единицы)
    sort2.CommandText = "select Id, Name, Price, In_Stock, Category from ProductOffer order by In_Stock * Price";
    var reader2 = sort2.ExecuteReader();
    Console.WriteLine("Cписок товаров, отсортированных по своей суммарной стоимости на складе:");
    while (reader2.Read())
    {
        Console.WriteLine("{0} {1} {2} {3} {4}", reader2.GetInt32(0), reader2.GetString(1), reader2.GetDouble(2), reader2.GetInt32(3), reader2.GetInt32(4));
    }
    reader2.Close();

    Console.WriteLine();
    var select1 = connection.CreateCommand();//Вывести список товаров, стоимостью “ниже среднего по базе”
    select1.CommandText = "select Id, Name, Price, In_Stock, Category from ProductOffer where Price < (select sum(price)/count(price) from ProductOffer)";
    var reader3 = select1.ExecuteReader();
    Console.WriteLine("Cписок товаров, стоимостью “ниже среднего по базе”:");
    while (reader3.Read())
    {
        Console.WriteLine("{0} {1} {2} {3} {4}", reader3.GetInt32(0), reader3.GetString(1), reader3.GetDouble(2), reader3.GetInt32(3), reader3.GetInt32(4));
    }
    reader3.Close();

    Console.WriteLine();
    var sum1 = connection.CreateCommand();//Вывести пары: {группа товара, число единиц товара на складе в группе}, использовать группировку (GroupBy, Sum)
    sum1.CommandText = "select Category, sum (In_Stock) from ProductOffer group by Category";
    var reader4 = sum1.ExecuteReader();
    Console.WriteLine("Пары: {группа товара, число единиц товара на складе в группе}:");
    while (reader4.Read())
    {
        Console.WriteLine("{0}, {1}", reader4.GetInt32(0), reader4.GetInt32(1));
    }
    reader4.Close();

    var min1 = connection.CreateCommand(); //Извлечь товар с минимальной ценой, увеличить её на 20%. Результат сохранить
    min1.CommandText = "update ProductOffer set Price = Price * 1.2 where Price = (SELECT MIN(Price) FROM ProductOffer)";
    min1.ExecuteNonQuery();

    var max1 = connection.CreateCommand(); //Найти товар с максимальной ценой и удалить его из базы.
    max1.CommandText = "DELETE FROM ProductOffer WHERE Price = (SELECT Max(Price) FROM ProductOffer);";
    max1.ExecuteNonQuery();

    var min2 = connection.CreateCommand(); //Найти категорию с наименьшим числом товарных позиций. Добавить в неё новую товарную позицию
    min2.CommandText = "select Category, Coun = count (Category) from ProductOffer group by Category";
    var reader5 = min2.ExecuteReader();
    var minCategory = 1;
    var minSum = 2147483647;
    while (reader5.Read())
    {
        if (minSum > reader5.GetInt32(1))
        {
            minSum = reader5.GetInt32(1);
            minCategory = reader5.GetInt32(0);
        }
    }
    reader5.Close();
    var min3 = connection.CreateCommand();
    min3.CommandText = "insert into ProductOffer (Id, Category, In_Stock, Name, Price) values (10, " + minCategory.ToString() + ", 432, 'Huawei', 21999);";
    min3.ExecuteNonQuery();

    connection.Close();


}

static void Entity()//использование Entity Framework
{
    Console.WriteLine("--------------------------------------------------------------------------------------------");
    Console.WriteLine("Entity Framework");
    Console.WriteLine();
    OnlineStoreContext db = new OnlineStoreContext(); 
    db.CreateDbIfNotExists(); 
    var sort1 = db.ProductOffers. //Вывести список товаров, отсортированных по названию своей группы, далее по цене.
        Join(db.CategoryTables,
             prod => prod.Category,
             cat => cat.Category,
             (prod, cat) => new { Id = prod.Id, Name = prod.Name, Price = prod.Price, InStock = prod.InStock, Category = cat.CategoryName }).
        OrderBy(x => x.Category).ThenBy(x => x.Price).ToList();
    Console.WriteLine("Список товаров, отсортированных по названию своей группы, далее по цене:");
    foreach (var a in sort1)
    {
        Console.Write(a.Id);
        Console.Write(" ");
        Console.Write(a.Price);
        Console.Write(" ");
        Console.Write(a.Name);
        Console.Write(" ");
        Console.Write(a.InStock);
        Console.Write(" ");
        Console.WriteLine(a.Category);
    }

    Console.WriteLine();
    var sort2 = db.ProductOffers.
        OrderBy(x => x.Price * x.InStock).ToList(); //Вывести список товаров, отсортированных по своей суммарной стоимости на складе (единиц товара * цена единицы)
    Console.WriteLine("Cписок товаров, отсортированных по своей суммарной стоимости на складе:");
    foreach (var a in sort2)
    {
        Console.Write(a.Id);
        Console.Write(" ");
        Console.Write(a.Price);
        Console.Write(" ");
        Console.Write(a.Name);
        Console.Write(" ");
        Console.Write(a.InStock);
        Console.Write(" ");
        Console.WriteLine(a.Category);
    }

    Console.WriteLine();
    var sum = db.ProductOffers. //Вывести список товаров, стоимостью “ниже среднего по базе”
        Sum(x => x.Price);
    var count = db.ProductOffers.
        Count();
    var priceBase = sum / count;
    var select1 = db.ProductOffers.
        Where(s => s.Price < priceBase).ToList();
    Console.WriteLine("Cписок товаров, стоимостью “ниже среднего по базе”:");
    foreach (var a in select1)
    {
        Console.Write(a.Id);
        Console.Write(" ");
        Console.Write(a.Price);
        Console.Write(" ");
        Console.Write(a.Name);
        Console.Write(" ");
        Console.Write(a.InStock);
        Console.Write(" ");
        Console.WriteLine(a.Category);
    }

    Console.WriteLine();
    var sum1 = db.ProductOffers. //Вывести пары: {группа товара, число единиц товара на складе в группе}, использовать группировку (GroupBy, Sum)
        GroupBy(x => x.Category).
        Select(y => new {Category = y.Key, Sum = y.Sum(a => a.InStock).ToString() }).ToList();
    Console.WriteLine("Пары: {группа товара, число единиц товара на складе в группе}:");
    foreach (var a in sum1)
    {
        Console.WriteLine(a);
    }

   var min = db.ProductOffers.Min(x => x.Price); //Извлечь товар с минимальной ценой, увеличить её на 20%. Результат сохранить
   var minProduct = db.ProductOffers.Where(s => s.Price == min).ToList();
   minProduct.ForEach(x => x.Price = min * 1.2);

   var max = db.ProductOffers.Max(x => x.Price); //Найти товар с максимальной ценой и удалить его из базы.
   var maxProduct = db.ProductOffers.Where(s => s.Price == max).ToList();
   maxProduct.ForEach(x => db.ProductOffers.Remove(x));

   var minCategoryList = db.ProductOffers. //Найти категорию с наименьшим числом товарных позиций. Добавить в неё новую товарную позицию
        GroupBy(x => x.Category).
        Select(x => new {Category = x.Key, Sum = x.Select(l => l.Category).Count() }).ToList();
    var minCategory = 1;
    var minSum = 2147483647;
    foreach (var a in minCategoryList)
    {
        if (a.Sum < minSum)
        {
            minCategory = a.Category;
            minSum = a.Sum;
        }
    }
    db.Add(new ProductOffer { Id = 11, Category = minCategory, InStock = 332, Name = "Xiaomi", Price = 25999 });

    db.SaveChanges();
}

static void TableOut()
{
    var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;");
    connection.Open();

    var sort1 = connection.CreateCommand(); //Вывести список товаров, отсортированных по названию своей группы, далее по цене.
    sort1.CommandText = "select Id, Name, Price, In_Stock, Category from ProductOffer;";
    var reader1 = sort1.ExecuteReader();
    Console.WriteLine("Список товаров:");
    while (reader1.Read())
    {
        Console.WriteLine("{0} {1} {2} {3} {4}", reader1.GetInt32(0), reader1.GetString(1), reader1.GetDouble(2), reader1.GetInt32(3), reader1.GetInt32(4));
    }
    reader1.Close();
    connection.Close();
}

static void TableToDefault()
{
    var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;");
    connection.Open();

    var max1 = connection.CreateCommand(); //Найти товар с максимальной ценой и удалить его из базы.
    max1.CommandText = "DELETE FROM ProductOffer;";
    max1.ExecuteNonQuery();

    var max2 = connection.CreateCommand(); //Найти товар с максимальной ценой и удалить его из базы.
    max2.CommandText = "insert into ProductOffer(Price, Name, In_Stock, Category, Id) values (24999,'Honor X8',200,1,1), (49999,'Apple iPhone 11',190,1,2), (12499,'Samsung Galaxy M12',300,1,3), (59999,'Honor MagicBook X 15',275,2,4), (139999,'Apple MacBook Air 13',90,2,5), (49999,'HP 15s-eq2086ur',230,	2,6), (21999,'HUAWEI MatePad 10.4',190,3,7), (18499,'Lenovo M10 FHD',345,3,8), (20999,'Nokia T20 SS',315,3,9); ";
    max2.ExecuteNonQuery();

    connection.Close();
}

static void Main()
{
    int choosing = 1;
    while (choosing != 0)
    {
        Console.WriteLine("Выберите действие: ");
        Console.WriteLine("1. Выполнить команды AdoNet");
        Console.WriteLine("2. Выполнить команды Entity Framework");
        Console.WriteLine("3. Вывести таблицу ProductOffer");
        Console.WriteLine("4. Вернуть таблицу ProductOffer к стандартным значениям");
        Console.WriteLine("0. Выход");
        choosing = Convert.ToInt32(Console.ReadLine());
        switch (choosing)
        {
            case 1:
                AdoNet();
                break;
            case 2:
                Entity();
                break;
            case 3:
                TableOut();
                break;
            case 4:
                TableToDefault();
                break;
            case 0:
                System.Environment.Exit(1);
                break;
            default:
                break;


        }
    }
}

Main();