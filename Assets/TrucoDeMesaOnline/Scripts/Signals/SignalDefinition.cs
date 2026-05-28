namespace TrucoDeMesaOnline
{
    public static class SignalDefinition
    {
        public static string GetLabel(SignalType signal)
        {
            switch (signal)
            {
                case SignalType.WinkZap:
                    return "Piscadela / Zap";
                case SignalType.RaiseEyebrowHearts:
                    return "Sobrancelha / Copas";
                case SignalType.PuffCheekSpades:
                    return "Bochecha / Espadas";
                case SignalType.TongueOrNoseDiamonds:
                    return "Lingua ou nariz / Ouros";
                case SignalType.OneShoulderThree:
                    return "Um ombro / 3";
                case SignalType.BothShouldersDoubleThree:
                    return "Dois ombros / Dois 3";
                case SignalType.NoseTouchAce:
                    return "Mao no nariz / As";
                case SignalType.ChinHandKing:
                    return "Mao no queixo / Rei";
                case SignalType.EarHandQueen:
                    return "Mao na orelha / Dama";
                case SignalType.EarToChinJack:
                    return "Orelha ao queixo / Valete";
                default:
                    return signal.ToString();
            }
        }

        public static string GetConsoleText(SignalType signal)
        {
            return GetLabel(signal) + " (pode ser verdadeiro ou blefe)";
        }
    }
}

