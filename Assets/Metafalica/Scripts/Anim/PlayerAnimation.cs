using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metafalica.RPG
{
    public class PlayerAnimation : BaseAnimation
    {
        public Transform rightHandTransform;
        private Transform weaponDefaultParent;
        private Transform _weapon;
        private Vector3 offsetPos;
        private Quaternion offsetRot;

        protected override void Start()
        {
            base.Start();

            offsetPos = new Vector3(-0.1f, 0, 0);
            offsetRot = Quaternion.Euler(new Vector3(180, 0, 0));
            weaponDefaultParent = PlayerManager.Instance.Player.transform.Find("Weapon");
            _weapon = weaponDefaultParent.GetChild(0);
        }

        private void Update()
        {
        }

        public void WeaponAppear()
        {
            _weapon.SetParent(rightHandTransform);
            _weapon.localPosition = offsetPos;
            _weapon.localRotation = offsetRot;
            _weapon.localScale = Vector3.one;
            _weapon.gameObject.SetActive(true);
        }

        public void WeaponHide()
        {
            _weapon.SetParent(weaponDefaultParent);
            _weapon.localPosition = Vector3.zero;
            _weapon.localRotation = Quaternion.identity;
            _weapon.localScale = Vector3.one;
            _weapon.gameObject.SetActive(false);
        }
    }
}