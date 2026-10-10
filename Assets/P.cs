using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public interface Target
{
    public void onHit(
    ContactPoint contact,
    Arrow arrow);
}

public class P : MonoBehaviour
{
    static public Transform player, camera;

    static public AudioSource[] auso;
    Keyboard k;

    [Header("Serial")]
    SerialPort serial;
    private const int PacketSize = 38;


    public static bool toStart = false;

    [Header("Received Data")]

    public static int[] idata;
    public static float[] fdata;

    public static bool stickbtn;

    public static byte vib = 0;
    byte prevvib = 0;

    byte[] temp = new byte[PacketSize];

    private void Awake()
    {
        idata = new int[2];
        fdata = new float[7];
        player = transform;
    }

    void Start()
    {
        k = Keyboard.current;
        auso = GetComponents<AudioSource>();

        try
        {
            serial = new SerialPort(Config.port, 115200)
            {
                ReadTimeout = 10,
                WriteTimeout = 10
            };

            serial.Open();
            Debug.Log($"Serial connected: {Config.port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Serial open failed: {e.Message}");
        }
    }

    void FixedUpdate()
    {
        if (k.sKey.wasPressedThisFrame) SceneManager.LoadScene("Config");
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            // 완전한 패킷 하나 이상 있을 때만 헤더 탐색
            while (serial.BytesToRead >= PacketSize)
            {

                // 첫 번째 헤더 탐색
                if (serial.ReadByte() != 0xAA) continue;

                temp[0] = 0xAA;

                // 나머지 37바이트 읽기
                int totalRead = 1;

                while (totalRead < PacketSize)
                {
                    int readCount = serial.Read(
                        temp,
                        totalRead,
                        PacketSize - totalRead
                    );

                    totalRead += readCount;
                }

                switch (temp[37])
                {
                    case 0:
                    stickbtn = true;
                    break;

                    case 1:
                    stickbtn = false;
                    break;

                    default:
                    continue;

                }

                
                for(int i = 0; i<7; i++)
                {
                    fdata[i] = BitConverter.ToSingle(temp, 1+i*4);   
                }
                idata[0] = BitConverter.ToInt32(temp, 33);
                idata[1] = BitConverter.ToInt32(temp, 25);

                toStart = true;
                // 이번 FixedUpdate에서는 한 패킷만 사용
                break;
            }

            if (vib != prevvib)
            {
                prevvib = vib;
                SetVibration(vib);
            }
        }
        catch (TimeoutException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"Serial read failed: {e.Message}");
        }
    }

    public void SetVibration(byte value)
    {
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            serial.Write(new[] { value }, 0, 1);
        }
        catch (Exception e)
        {
            Debug.LogError($"Serial write failed: {e.Message}");
        }
    }

    private void OnDestroy()
    {
        CloseSerial();
    }

    private void OnApplicationQuit()
    {
        CloseSerial();
    }

    private void CloseSerial()
    {
        if (serial == null)
            return;

        if (serial.IsOpen)
            serial.Close();

        serial.Dispose();
        serial = null;
    }
}

public class SC{
    public float value, target;
    
    int e = 0;
    float delta;

    readonly float rest;
    readonly float alpha;
    readonly float ratio;
    readonly float err;
    readonly int reas;
    readonly float[] queue;
    readonly float[] queori;

    public SC(float initval, float ratio, int reasonance, float error)
    {
        target = value = initval;
        this.ratio = ratio;
        reas = reasonance;
        queue = new float[reasonance];
        queori = new float[reasonance];
        rest = Mathf.Pow(ratio, reas);
        err = error;
        alpha = 1-ratio;
    }
    public void update(float target)
    {
        if(Mathf.Abs(target - value) < err)
        {
            value = target;
            Array.Clear(queori, 0, reas);
            return;
        }
        if(e == reas) e = 0;
        delta = queori[e] * rest + (queue[e] = (queori[e++] = -this.target + (this.target = target)) * alpha);
        
        for(int i = 0; i<reas; i++)
        {
            if(queue[i] == 0) continue;
            delta += queue[i] *= ratio; 
        }
        if(delta == 0) value = target;
        else value += delta;
    }
    public void update()
    {
        update(target);
    }
}