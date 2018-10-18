using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Loader
{
    public class DataLoader
    {
        #region Private parameters

        #region Consts

        private const int CapacityOfKnapsackIndex = 4;

        private const int DimensionIndex = 2;

        private const int MaxSpeedIndex = 6;

        private const int MinSpeedIndex = 5;

        private const int NumberOfItemsIndex = 3;

        private const string PathToFolderWithData = @"D:\7 semestr\Metaheurystyki\Data\ttp_student";

        private const string PathToTestData = @"D:\7 semestr\Metaheurystyki\Data\ttp_student\trivial_0.ttp";

        private const int PlacesStartIndex = 10;

        private const int RentingRatioIndex = 7;

        /// <summary>
        /// ValueIndex = 1; means that at in string[] numerical value is at ValueIndex position
        /// </summary>
        private const int ValueIndex = 1;

        #endregion Consts

        public int ItemsStartIndex => PlacesStartIndex + NumberOfPlaces + 1;
        public int NumberOfPlaces { get; set; }

        #region Item

        public const int ItemPlaceIdIndex = 3;
        private const int ItemIdIndex = 0;
        private const int PositionXIndex = 1;
        private const int PositionYIndex = 2;

        #endregion Item

        #region Place

        public const int ItemIdIndexForPlace = 3;
        public const int PlaceIdIndex = 0;
        public const int ProfitXIndex = 2;
        public const int WeightIndex = 1;

        #endregion Place

        #endregion Private parameters

        public async Task<DataContainer> GetCreatedDataContainerFromFileAsync(string filePath = PathToTestData)
        {
            var allLines = await ReadAllLinesAsync(filePath);

            return CreateDataContainer(allLines);
        }

        private async Task<string[]> ReadAllLinesAsync(string filePath = PathToTestData)
        {
            var fileContent = await File.ReadAllLinesAsync(filePath);
            return fileContent;
        }

        private DataContainer CreateDataContainer(string[] allLinesFromFile)
        {
            DataContainer dataContainer = new DataContainer
            {
                Dimension = GetIntFromLine(allLinesFromFile.ElementAt(DimensionIndex), ValueIndex),
                NumberOfItems = GetIntFromLine(allLinesFromFile.ElementAt(NumberOfItemsIndex), ValueIndex),
                CapacityOfKnapsack = GetIntFromLine(allLinesFromFile.ElementAt(CapacityOfKnapsackIndex), ValueIndex),

                MinSpeed = GetDoubleFromLine(allLinesFromFile.ElementAt(MinSpeedIndex), ValueIndex),
                MaxSpeed = GetDoubleFromLine(allLinesFromFile.ElementAt(MaxSpeedIndex), ValueIndex),
                RentingRatio = GetDoubleFromLine(allLinesFromFile.ElementAt(RentingRatioIndex), ValueIndex),
            };

            var places = CreatePlaces(allLinesFromFile, PlacesStartIndex, dataContainer.Dimension);
            NumberOfPlaces = dataContainer.Dimension;

            var items = CreateItems(allLinesFromFile, ItemsStartIndex, dataContainer.NumberOfItems);

            dataContainer.Places = places;
            dataContainer.Items = items;
            dataContainer.DistanceMatrix = CreateDistanceMatrix(places);
            dataContainer.ItemsVector = CreateItemsVector(items);

            return dataContainer;
        }

        private double[,] CreateDistanceMatrix(List<Place> places)
        {
            int placesCount = places.Count;
            double[,] distanceMatrix = new double[placesCount, placesCount];

            for (int i = 0; i < placesCount; i++)
            {
                for (int j = 0; j < placesCount; j++)
                {
                    distanceMatrix[i, j] = EuclideanDistance.FindEuclideanDistance(places.ElementAt(i), places.ElementAt(j));
                }
            }

            return distanceMatrix;
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

                #region id decreasing by 1 because at input file it id starts by index = 1. For this implementation necessary to start index by 0.

                itemId--;
                itemPlaceId--;

                #endregion id decreasing by 1 because at input file it id starts by index = 1. For this implementation necessary to start index by 0.

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

        private List<Place> CreatePlaces(string[] allLinesFromFile, int firstPlaceIndex, int numberOfPlaces)
        {
            List<Place> places = new List<Place>();
            var counter = numberOfPlaces + firstPlaceIndex;
            for (int i = firstPlaceIndex; i < counter; i++)
            {
                var line = allLinesFromFile.ElementAt(i);

                var placeId = GetIntFromLine(line, PlaceIdIndex);
                var placePozitionX = GetDoubleFromLine(line, PositionXIndex);
                var placePozitionY = GetDoubleFromLine(line, PositionYIndex);

                #region id decreasing by 1 because at input file it id starts by index = 1. For this implementation necessary to start index by 0.

                placeId--;

                #endregion id decreasing by 1 because at input file it id starts by index = 1. For this implementation necessary to start index by 0.

                Place tmpPlace = new Place()
                {
                    Id = placeId,
                    PozitionX = placePozitionX,
                    PozitionY = placePozitionY
                };

                places.Add(tmpPlace);
            }

            return places;
        }

        private double GetDoubleFromLine(string line, int indexAtSplitedLine)
        {
            var splitedLine = line.Split("\t");

            return Double.Parse(splitedLine[indexAtSplitedLine]);
        }

        private int GetIntFromLine(string line, int indexAtSplitedLine)
        {
            var splitedLine = line.Split("\t");

            return Int32.Parse(splitedLine[indexAtSplitedLine]);
        }
    }
}