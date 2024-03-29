﻿using UnityEngine;
using System.Collections.Generic;

namespace LowPolyWater
{
    public class LowPolyWater : MonoBehaviour
    {
        public float waveHeight = 0.5f;
        public float waveFrequency = 0.5f;
        public float waveLength = 0.75f;
        public AudioClip waterPlop;
        private List<Rigidbody> rigidbodies = new List<Rigidbody>();

        //Position where the waves originate from
        public Vector3 waveOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        MeshFilter meshFilter;
        Mesh mesh;
        Vector3[] vertices;

        private void Awake()
        {
            //Get the Mesh Filter of the gameobject
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            CreateMeshLowPoly(meshFilter);
        }

        /// <summary>
        /// Rearranges the mesh vertices to create a 'low poly' effect
        /// </summary>
        /// <param name="mf">Mesh filter of gamobject</param>
        /// <returns></returns>
        MeshFilter CreateMeshLowPoly(MeshFilter mf)
        {
            mesh = mf.sharedMesh;

            //Get the original vertices of the gameobject's mesh
            Vector3[] originalVertices = mesh.vertices;

            //Get the list of triangle indices of the gameobject's mesh
            int[] triangles = mesh.triangles;

            //Create a vector array for new vertices 
            Vector3[] vertices = new Vector3[triangles.Length];
            
            //Assign vertices to create triangles out of the mesh
            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = originalVertices[triangles[i]];
                triangles[i] = i;
            }

            //Update the gameobject's mesh with new vertices
            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            this.vertices = mesh.vertices;

            return mf;
        }
        
        void Update()
        {
            GenerateWaves();
        }

       
        private void FixedUpdate()
        {
           for (int i = 0; i < rigidbodies.Count; i++)
            {
                if (rigidbodies[i] == null)
                {
                    rigidbodies.Remove(rigidbodies[i]);
                    break;
                }

                if (rigidbodies[i].transform.position.y > transform.position.y)
                {
                    rigidbodies[i].AddForce(Physics.gravity);
                    rigidbodies[i].drag = 1;
                    rigidbodies.Remove(rigidbodies[i]);
                }
                else
                {
                    if(rigidbodies[i].name == "bobber")
                    {
                        rigidbodies[i].AddForce(-1 * Physics.gravity);
                        rigidbodies[i].drag = 2;
                    }
                    else
                    {
                        rigidbodies[i].AddForce(-.5f * Physics.gravity + new Vector3(0, 0.1f, 0));
                        rigidbodies[i].drag = 10;
                    }
                }
            }
        }
        
        /// <summary>
        /// Based on the specified wave height and frequency, generate 
        /// wave motion originating from waveOriginPosition
        /// </summary>
        void GenerateWaves()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];

                //Initially set the wave height to 0
                v.y = 0.0f;

                //Get the distance between wave origin position and the current vertex
                float distance = Vector3.Distance(v, waveOriginPosition);
                distance = (distance % waveLength) / waveLength;

                //Oscilate the wave height via sine to create a wave effect
                v.y = waveHeight * Mathf.Sin(Time.time * Mathf.PI * 2.0f * waveFrequency
                + (Mathf.PI * 2.0f * distance));
                
                //Update the vertex
                vertices[i] = v;
            }

            //Update the mesh properties
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }

        public void RemoveFromWater(Rigidbody rb)
        {
            rigidbodies.Remove(rb);
        }

        private void OnTriggerEnter(Collider other)
        {

            Debug.Log("Other: " + other.name);
            if (!other.name.Equals("fish1(Clone)") && !other.name.Equals("basemap_final") && !rigidbodies.Contains(other.attachedRigidbody)) {
                AudioSource.PlayClipAtPoint(waterPlop, other.transform.position);
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            Debug.Log("Name2: " + otherRb.name);

            if (otherRb == null)
            {
                Debug.Log("oops");
                return;
            }

            rigidbodies.Add(otherRb);
            Debug.Log("Count: " + rigidbodies.Count);
        }
        
        /*
        private void OnTriggerExit(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;

            if (otherRb == null)
            {
                return;
            }

            rigidbodies.Remove(otherRb);
        }
        */

    }
}