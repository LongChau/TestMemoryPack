using MemoryPack;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TestMemoryPack
{
    public class Test : SerializedMonoBehaviour
    {
        [SerializeField] List<QuestData> questDatas = new List<QuestData>();
        [SerializeField] List<QuestData> deserizliedQuestDatas = new List<QuestData>();

        // Start is called before the first frame update
        //IEnumerator Start()
        //{
        //    Application.targetFrameRate = 30;
        //    //MyPerson.RegisterFormatter(); 
        //    //var person = new MyPerson { Age = 9999, Name = "ACB" };
        //    //bool check = MemoryPackFormatterProvider.IsRegistered<MyPerson>();
        //    //var bin = MemoryPackSerializer.Serialize(person);
        //    //Debug.Log("Payload size:" + bin.Length);
        //    //var v2 = MemoryPackSerializer.Deserialize<MyPerson>(bin);
        //    //Debug.Log("OK Deserialzie:" + v2.Age + ":" + v2.Name);

        //    TestSerialized(SerializedType.Newton);
        //    yield return new WaitForSecondsRealtime(1.0f);
        //    TestSerialized(SerializedType.Odin);
        //    yield return new WaitForSecondsRealtime(1.0f);
        //    TestSerialized(SerializedType.Memory);
        //}

        [Button]
        void TestNewton()
        {
            TestSerialized(SerializedType.Newton);
            Debug.Break();
        }
        [Button]
        void TestOdin()
        {
            TestSerialized(SerializedType.Odin);
            Debug.Break();
        }
        [Button]
        void TestMemory()
        {
            TestSerialized(SerializedType.Memory);
            Debug.Break();
        }

        [Button]
        void CreateQuestData()
        {
            for (int i = 0; i < 300; i++)
            {
                questDatas.Add(new QuestData()
                {
                    mapId = i.ToString(),
                    currentStep = i,
                    questValueDic = new Dictionary<int, QuestGoal>
                    {
                        { 1, new QuestGoal() { goldBonusFromSkip = 0, hasComplete = false, value = 0 } },
                        { 2, new QuestGoal() { goldBonusFromSkip = 0, hasComplete = false, value = 0 } },
                        { 3, new QuestGoal() { goldBonusFromSkip = 0, hasComplete = false, value = 0 } }
                    }
                });
            }
        }

        [Button]
        void TestSerialized(SerializedType type)
        {
            MemoryPackFormatterProvider.IsRegistered<QuestData>();

            // Register MyClass with MemoryPack
            //MemoryPackFormatterProvider.Register<QuestData>();


            deserizliedQuestDatas.Clear();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string json = "";
            byte [] data = default(byte[]);

            switch (type)
            {
                case SerializedType.Newton:
                    json = JsonConvert.SerializeObject(questDatas);
                    PlayerPrefs.SetString("QUEST_DATA", json);
                    break;
                case SerializedType.Odin:
                    data = SerializationUtility.SerializeValue(questDatas, DataFormat.JSON);
                    json = Encoding.ASCII.GetString(data);
                    PlayerPrefs.SetString("QUEST_DATA", json);
                    break;
                case SerializedType.Memory:
                    data = MemoryPackSerializer.Serialize(questDatas);
                    //json = Encoding.ASCII.GetString(data);
                    json = Convert.ToBase64String(data);
                    PlayerPrefs.SetString("QUEST_DATA", json);
                    break;
                default:
                    break;
            }
            

            stopwatch.Stop();
            UnityEngine.Debug.Log(type + " Serialized runs for " + stopwatch.Elapsed.TotalSeconds);

            stopwatch.Start();
            string encode = "";
            switch (type)
            {
                case SerializedType.Newton:
                    json = PlayerPrefs.GetString("QUEST_DATA");
                    deserizliedQuestDatas = JsonConvert.DeserializeObject<List<QuestData>>(json);
                    break;
                case SerializedType.Odin:
                    encode = PlayerPrefs.GetString("QUEST_DATA");
                    data = Encoding.ASCII.GetBytes(encode);
                    deserizliedQuestDatas = SerializationUtility.DeserializeValue<List<QuestData>>(data, DataFormat.JSON);
                    break;
                case SerializedType.Memory:
                    encode = PlayerPrefs.GetString("QUEST_DATA");
                    //data = Encoding.ASCII.GetBytes(encode);
                    data = Convert.FromBase64String(encode);
                    deserizliedQuestDatas = MemoryPackSerializer.Deserialize<List<QuestData>>(data);
                    break;
                default:
                    break;
            }

            stopwatch.Stop();
            UnityEngine.Debug.Log(type + " Deserialized runs for " + stopwatch.Elapsed.TotalSeconds);
        }
    }

    public enum SerializedType
    {
        Newton,
        Odin,
        Memory,
    }

    [MemoryPackable]
    public partial class QuestData
    {
        public string mapId;
        public int currentStep;
        public Dictionary<int, QuestGoal> questValueDic = new Dictionary<int, QuestGoal>();
    }

    [MemoryPackable]
    public partial class QuestGoal
    {
        public int value;
        public bool hasComplete;
        public int goldBonusFromSkip;
    }
}

    [MemoryPackable]
    public partial class MyPerson
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }
