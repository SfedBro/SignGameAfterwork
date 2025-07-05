import os
from os import listdir
from os.path import isfile, join
import numpy as np
import tensorflow as tf
from tensorflow.keras import layers, models, callbacks, regularizers
from sklearn.model_selection import train_test_split
from PIL import Image
import matplotlib.pyplot as plt
import tf2onnx
import onnx
from keras.models import load_model
import cv2
import json

class CNNModel:
    #initialize model with threshlod of 0.7
    def __init__(self, input_shape, num_classes):
        self.input_shape = input_shape
        self.num_classes = num_classes
        self.model = self._build_model()
        self.confidence_threshold = 0.7  
    #saving in onnx to proccess in C#
    def save_as_onnx(self, model_path, output_onnx_path):
        model = tf.keras.models.load_model(model_path)
        model.output_names=['output']
        input_signature = [tf.TensorSpec(model.inputs[0].shape, model.inputs[0].dtype)]
        onnx_model, _ = tf2onnx.convert.from_keras(model, input_signature,opset=9) 
        onnx.checker.check_model(onnx_model)
        onnx.save(onnx_model, output_onnx_path)

    #building a model    
    def _build_model(self):
        #3 CNN layers with batch and dropout
        #also have a dense layer for output
        model = models.Sequential([
            layers.Conv2D(32, (3, 3), activation='relu', 
                         kernel_regularizer=regularizers.l2(0.001),
                         input_shape=self.input_shape),
            layers.BatchNormalization(),
            layers.MaxPooling2D((2, 2)),
            layers.Dropout(0.2),
            
            layers.Conv2D(64, (3, 3), activation='relu',
                         kernel_regularizer=regularizers.l2(0.001)),
            layers.BatchNormalization(),
            layers.MaxPooling2D((2, 2)),
            layers.Dropout(0.3),
            
            layers.Conv2D(128, (3, 3), activation='relu',
                         kernel_regularizer=regularizers.l2(0.001)),
            layers.BatchNormalization(),
            layers.MaxPooling2D((2, 2)),
            layers.Dropout(0.4),
            
            layers.Flatten(),
            layers.Dense(256, activation='relu',
                       kernel_regularizer=regularizers.l2(0.001)),
            layers.BatchNormalization(),
            layers.Dropout(0.5),
            
            layers.Dense(self.num_classes, activation='softmax')
        ])
        #use adam optmizier
        optimizer = tf.keras.optimizers.Adam(learning_rate=0.0005)
        
        model.compile(optimizer=optimizer,
                    loss='sparse_categorical_crossentropy',
                    metrics=['accuracy'])
        
        return model
    #training
    def train(self, X_train, y_train, X_val, y_val, epochs=100):
        checkpoint_path = "static_ml/model.h5"
        #have chekpoints for safety
        callbacks_list = [
            callbacks.ModelCheckpoint(checkpoint_path, 
                                    monitor='val_accuracy',
                                    save_best_only=True,
                                    mode='max'),
            callbacks.ReduceLROnPlateau(monitor='val_loss', factor=0.2, patience=5)
        ]
        
        history = self.model.fit(
            X_train, y_train,
            epochs=epochs,
            batch_size=32,
            validation_data=(X_val, y_val),
            callbacks=callbacks_list,
            verbose=1
        )
        
        return history, checkpoint_path
    #to return confidense of eachclass
    def predict_with_confidence(self, X):
        probs = self.model.predict(X, verbose=0)
        predictions = np.argmax(probs, axis=1)
        confidences = np.max(probs, axis=1)
        
        mask = confidences >= self.confidence_threshold
        confident_predictions = predictions[mask]
        confident_confidences = confidences[mask]
        confident_indices = np.where(mask)[0]
        
        return confident_predictions, confident_confidences, confident_indices
    
    def evaluate_high_confidence(self, X_test, y_test):
        preds, confs, indices = self.predict_with_confidence(X_test)
        if len(indices) == 0:
            print("No predictions met the 70% confidence threshold")
            return 0.0
        
        confident_X = X_test[indices]
        confident_y = y_test[indices]
        
        accuracy = np.mean(preds == confident_y)
        coverage = len(indices) / len(X_test)
        
        print(f"\nHigh-Confidence Evaluation (threshold: {self.confidence_threshold:.0%})")
        print(f"Accuracy on confident predictions: {accuracy:.4f}")
        print(f"Coverage (fraction of samples predicted): {coverage:.4f}")
        
        return accuracy, coverage
#loading and preprocessing dataset
def load_dataset(data_dir):
    images = []
    labels = []
    #find all classes
    names = [f[:f.find('.')] for f in listdir('static_ml\etalon') if isfile(join('static_ml\etalon', f))]
    enum = enumerate(names)
    class_names = {}
    for names, cnt in enum:
        class_names[cnt] = names
    print(class_names)
        
    with open('static_ml\CNNModelC#\classes.json', 'w') as f:
        json.dump(class_names, f)
        
    for filename in listdir(data_dir):
        if filename.endswith('.png'):
            if filename.startswith('x'):
                continue
            else:
                label = filename[:filename.find('-')]
            #preproccess to grayscale and resizing to 64 to 64 with scale
            img_path = os.path.join(data_dir, filename)
            img = cv2.imread(img_path)
            img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            img = cv2.resize(img, dsize = (64, 64) )
            img_array = np.array(img) / 255.0
            img_array = np.expand_dims(img_array, axis=-1)
            
            images.append(img_array)
            labels.append(class_names[label])
    return np.array(images), np.array(labels)
#plooting for analyze
def plot_history(history):
    plt.figure(figsize=(12, 5))
    
    plt.subplot(1, 2, 1)
    plt.plot(history.history['accuracy'], label='Train Accuracy')
    plt.plot(history.history['val_accuracy'], label='Validation Accuracy')
    plt.title('Accuracy over Epochs')
    plt.legend()
    
    plt.subplot(1, 2, 2)
    plt.plot(history.history['loss'], label='Train Loss')
    plt.plot(history.history['val_loss'], label='Validation Loss')
    plt.title('Loss over Epochs')
    plt.legend()
    
    plt.show()
    
if __name__ == "__main__":
    dataset_dir = "static_ml\images"
    X, y = load_dataset(dataset_dir)
    X_pre_train, X_test, y_pre_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
    X_train, X_val, y_train, y_val = train_test_split(X_pre_train,y_pre_train, test_size = 0.2, random_state= 42)
    hc_model = CNNModel(input_shape=X_train[0].shape, num_classes=len(np.unique(y)))
    history, model_path = hc_model.train(X_train, y_train, X_val, y_val, epochs=50)
    
    hc_model.save_as_onnx(model_path, "static_ml\CNNModelC#\CNN_model.onnx")
    
    plot_history(history)
    
    hc_model.evaluate_high_confidence(X_test, y_test)
    