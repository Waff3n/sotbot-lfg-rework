﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SeaOfThieves.Entities
{
    public static class InviterList
    {
        public static Dictionary<ulong, Inviter> Inviters = new Dictionary<ulong, Inviter>();

        public static void Update(Inviter inviter)
        {
            Inviters[inviter.InviterId] = inviter;
        }

        public static void SaveToXML(string fileName)
        {
            var doc = new XDocument();
            var root = new XElement("inviters");

            foreach (var inviter in Inviters.Values)
            {
                var dElement = new XElement("inviter");
                dElement.Add(new XElement("inviterId", inviter.InviterId));
                dElement.Add(new XElement("active", inviter.Active));
                foreach (var friend in inviter.Referrals) dElement.Add(new XElement("referral", friend));
                root.Add(dElement);
            }

            doc.Add(root);
            doc.Save(fileName);
        }

        public static void ReadFromXML(string fileName)
        {
            var doc = XDocument.Load(fileName);
            foreach (var inviter in doc.Element("inviters").Elements("inviter"))
            {
                var created = new Inviter(Convert.ToUInt64(inviter.Element("inviterId").Value),
                                          Convert.ToBoolean(inviter.Element("active").Value));
                foreach (var friend in inviter.Elements("referral")) created.AddReferral(Convert.ToUInt64(friend.Value));
            }
        }
    }

    public class Inviter
    {
        public Inviter(ulong id, bool active = true)
        {
            InviterId = id;
            Referrals = new List<ulong>();
            Active = true;

            InviterList.Inviters[InviterId] = this;
        }

        public ulong InviterId { get; }
        public List<ulong> Referrals { get; }
        public bool Active { get; private set; }

        public static Inviter Create(ulong inviterId)
        {
            InviterList.Update(new Inviter(inviterId));
            return new Inviter(inviterId);
        }

        public void AddReferral(ulong friend)
        {
            Referrals.Add(friend);

            InviterList.Inviters[InviterId] = this;
        }

        public void RemoveReferral(ulong friend)
        {
            Referrals.Remove(friend);

            InviterList.Inviters[InviterId] = this;
        }

        public void UpdateState(bool state)
        {
            Active = state;

            InviterList.Inviters[InviterId] = this;
        }

        public void Remove()
        {
            InviterList.Inviters.Remove(InviterId);
        }
    }
}
