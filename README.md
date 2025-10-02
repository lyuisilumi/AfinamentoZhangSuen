# 🧠 AfinamentoZhangSuen – Processamento de Imagens Digitais

Este projeto foi desenvolvido como parte da disciplina **TTC I – Tópicos em Tecnologia de Computação I**, com o objetivo de implementar algoritmos fundamentais de **processamento de imagens digitais** utilizando **C#** e manipulação direta de memória (**DMA – Direct Memory Access**).  
Todas as etapas foram desenvolvidas **sem uso de bibliotecas prontas** para as operações principais, permitindo um entendimento mais profundo do funcionamento interno dos algoritmos.

---

## 🎯 Objetivo do Projeto

O objetivo do trabalho é aplicar técnicas de processamento digital de imagens em três etapas principais sobre uma imagem contendo caracteres (`caracteres.png`):

1. **Afinamento (Thinning)** – Reduzir os objetos (caracteres) ao seu esqueleto, com apenas 1 pixel de espessura.  
2. **Extração de Contornos (Contour Following)** – Detectar e percorrer o contorno de cada objeto afinado.  
3. **Cálculo do Retângulo Mínimo** – Determinar o menor retângulo que engloba cada caractere detectado.

---

## 🧰 Tecnologias Utilizadas

- 💻 **Linguagem:** C#  
- 🛠️ **Framework:** .NET  
- 🖼️ **Manipulação de imagens:** `System.Drawing` e `Bitmap`  
- ⚙️ **DMA (Direct Memory Access):** Processamento direto dos pixels sem funções prontas  

---

## 🔍 Funcionalidades

- 📏 **Afinamento:** Converte caracteres em esqueletos com largura mínima.  
- 🔍 **Extração de Contornos:** Detecta e percorre o contorno de cada caractere na imagem.  
- 📐 **Retângulo Mínimo:** Calcula e desenha o menor retângulo que envolve cada caractere detectado.  

---

## 📊 Etapas do Processamento

1. **Entrada:**  
   - Imagem original (`caracteres.png`) com caracteres em preto sobre fundo branco.  

2. **Processamento:**  
   - Aplicação do algoritmo **Zhang-Suen** para afinamento.  
   - Aplicação do algoritmo **Contour Following** para extração dos contornos.  
   - Cálculo do **retângulo mínimo** para cada caractere.  

3. **Saída:**  
   - Imagem final com os caracteres contornados e delimitados por retângulos mínimos.
