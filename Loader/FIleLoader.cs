using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Loader
{
    public class FileLoader
    {
        #region Private parameters

        /// <summary>
        /// ValueIndex = 1; means that at in string[] numerical value is at ValueIndex position
        /// </summary>
        private const int ValueIndex = 1;

        private const int PlacesStartIndex = 10;
        public int ItemsStartIndex => PlacesStartIndex + NumberOfPlaces + 1;
        public int NumberOfPlaces { get; set; }

        private const int DimensionIndex = 2;
        private const int NumberOfItemsIndex = 3;
        private const int CapacityOfKnapsackIndex = 4;
        private const int MinSpeedIndex = 5;
        private const int MaxSpeedIndex = 6;
        private const int RentingRatioIndex = 7;

        #region Item

        private const int ItemIdIndex = 0;
        private const int PozitionXIndex = 1;
        private const int PozitionYIndex = 2;
        public const int ItemPlaceIdIndex = 3;

        #endregion Item

        #region Place

        public const int PlaceIdIndex = 0;
        public const int WeightIndex = 1;
        public const int ProfitXIndex = 2;
        public const int ItemIdIndexForPlace = 3;

        #endregion Place

        private const string PathToFolderWithData = @"D:\7 semestr\Metaheurystyki\Data\ttp_student";
        private const string PathToTestData = @"D:\7 semestr\Metaheurystyki\Data\ttp_student\trivial_0.ttp";

        #endregion Private parameters

        private int GetIntFromLine(string line, int indexAtSplitedLine)
        {
            var splitedLine = line.Split("\t");

            return Int32.Parse(splitedLine[indexAtSplitedLine]);
        }

        private double GetDoubleFromLine(string line, int indexAtSplitedLine)
        {
            var splitedLine = line.Split("\t");

            return Double.Parse(splitedLine[indexAtSplitedLine]);
        }

        private List<Place> CreatePlaces(string[] allLinesFromFile, int firstPlaceIndex, int numberOfPlaces)
        {
            List<Place> places = new List<Place>();
            var counter = numberOfPlaces + firstPlaceIndex;
            for (int i = firstPlaceIndex; i < counter; i++)
            {
                var line = allLinesFromFile.ElementAt(i);

                Place tmpPlace = new Place()
                {
                    Id = GetIntFromLine(line, PlaceIdIndex),
                    PozitionX = GetDoubleFromLine(line, PozitionXIndex),
                    PozitionY = GetDoubleFromLine(line, PozitionYIndex)
                };

                places.Add(tmpPlace);
            }

            return places;
        }

        private List<Item> CreateItems(string[] allLinesFromFile, int firstItemIndex, int numberOfItems)
        {
            List<Item> items = new List<Item>();
            var counter = numberOfItems + firstItemIndex;
            for (int i = firstItemIndex; i < counter; i++)
            {
                var line = allLinesFromFile.ElementAt(i);
                var itemId = GetIntFromLine(line, ItemIdIndex);
                var itemWeight = GetIntFromLine(line, WeightIndex);
                var itemProfit = GetIntFromLine(line, ProfitXIndex);
                var itemPlaceId = GetIntFromLine(line, ItemPlaceIdIndex);

                Item tmpItem = new Item()
                {
                    Id = itemId,
                    Weight = itemWeight,
                    Profit = itemProfit,
                    PlaceId = itemPlaceId
                };

                items.Add(tmpItem);
            }

            return items;
        }

        private DataContainer CreateDataContainer(string[] allLinesFromFile)
        {
            DataContainer DataContainer = new DataContainer
            {
                Dimension = GetIntFromLine(allLinesFromFile.ElementAt(FileLoader.DimensionIndex), ValueIndex),
                NumberOfItems = GetIntFromLine(allLinesFromFile.ElementAt(FileLoader.NumberOfItemsIndex), ValueIndex),
                CapacityOfKnapsack = GetIntFromLine(allLinesFromFile.ElementAt(FileLoader.CapacityOfKnapsackIndex), ValueIndex),

                MinSpeed = GetDoubleFromLine(allLinesFromFile.ElementAt(FileLoader.MinSpeedIndex), ValueIndex),
                MaxSpeed = GetDoubleFromLine(allLinesFromFile.ElementAt(FileLoader.MaxSpeedIndex), ValueIndex),
                RentingRatio = GetDoubleFromLine(allLinesFromFile.ElementAt(FileLoader.RentingRatioIndex), ValueIndex),
            };

            var places = CreatePlaces(allLinesFromFile, PlacesStartIndex, DataContainer.Dimension);
            NumberOfPlaces = DataContainer.Dimension;

            var items = CreateItems(allLinesFromFile, ItemsStartIndex, DataContainer.NumberOfItems);

            DataContainer.Places = places;
            DataContainer.Items = items;
            DataContainer.DistanseMatrix = CreateDimenstionMatrix(places);
            DataContainer.ItemsVector = CreateItemsVector(items);

            return DataContainer;
        }

        private double[] CreateItemsVector(List<Item> items)
        {
            int itemsCount = items.Count;
            double[] itemsVector = new double[itemsCount];

            for (int i = 0; i < itemsCount; i++)
            {
                itemsVector[i] = items.ElementAt(i).PlaceId;
            }

            return itemsVector;
        }

        private double[,] CreateDimenstionMatrix(List<Place> places)
        {
            int placesCount = places.Count;
            double[,] dimenstionMatrix = new double[placesCount, placesCount];

            for (int i = 0; i < placesCount; i++)
            {
                for (int j = 0; j < placesCount; j++)
                {
                    dimenstionMatrix[i, j] = EuclideanDistance.FindEuclideanDistance(places.ElementAt(i), places.ElementAt(j));
                }
            }

            return dimenstionMatrix;
        }

        public async Task<string[]> ReadAllLinesAsync(string filePath = PathToTestData)
        {
            var fileContent = await File.ReadAllLinesAsync(filePath);
            return fileContent;
        }

        public async Task<DataContainer> GetDataFromFileAsync()
        {
            var allLines = await ReadAllLinesAsync();

            return CreateDataContainer(allLines);
        }
    }
}